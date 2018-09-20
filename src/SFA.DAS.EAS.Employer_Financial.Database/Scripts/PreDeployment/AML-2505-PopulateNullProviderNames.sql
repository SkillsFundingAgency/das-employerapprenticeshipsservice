BEGIN TRY
BEGIN TRANSACTION

    ;With ALLProviders (UKprn, ProviderName, PeriodEnd, RowNum)
   AS
   (
       SELECT p.Ukprn,
           pm.ProviderName,
           p.PeriodEnd,
           ROW_NUMBER() OVER(PARTITION BY p.Ukprn
                                 ORDER BY p.PeriodEnd DESC) AS rowNum
      FROM employer_financial.Payment p
      INNER JOIN employer_financial.PaymentMetaData pm
            ON p.PaymentMetaDataId = pm.id
      WHERE pm.ProviderName IS NOT NULL
   ),
   LatestProviders (UKprn, ProviderName)
   AS
   (SELECT ap.UKprn, ap.ProviderName FROM ALLProviders ap WHERE rowNum = 1)

   UPDATE pm
   SET pm.ProviderName = upm.ReplacementProviderName
    FROM employer_financial.PaymentMetaData pm
   INNER JOIN employer_financial.Payment payment
       ON payment.PaymentMetaDataId = pm.id
   INNER JOIN
       (
           SELECT p.UKprn, lp.ProviderName as ReplacementProviderName
           FROM employer_financial.PaymentMetaData pm
           INNER JOIN employer_financial.Payment p
               ON p.PaymentMetaDataId = pm.id
           INNER JOIN LatestProviders lp
               ON lp.UKprn = p.UKprn
           GROUP BY p.UKprn, pm.ProviderName, lp.ProviderName
           HAVING pm.ProviderName IS NULL
       ) upm
   ON upm.UKprn = payment.UKprn
   AND pm.ProviderName IS NULL

END TRY
BEGIN CATCH
DECLARE @ErrorMsg nvarchar(max)
DECLARE @ErrorSeverity INT;
DECLARE @ErrorState INT;
SET @ErrorMsg = ERROR_NUMBER() + ERROR_LINE() + ERROR_MESSAGE()
SET @ErrorSeverity = ERROR_SEVERITY()
SET @ErrorState = ERROR_STATE()

IF @@TRANCOUNT > 0
    ROLLBACK TRANSACTION

RAISERROR(@ErrorMsg, @ErrorSeverity, @ErrorState)
END CATCH
IF @@TRANCOUNT > 0
 COMMIT TRANSACTION