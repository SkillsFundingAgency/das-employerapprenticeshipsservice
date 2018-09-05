BEGIN TRY
BEGIN TRANSACTION

    ;With ALLProviders (UKprn, ProviderName)
   AS
   (
       SELECT p.UKprn, MAX(pm.ProviderName) as ProviderName
       FROM employer_financial.Payment p
       INNER JOIN employer_financial.PaymentMetaData pm
           ON p.PaymentMetaDataId = pm.id
       GROUP BY p.UKprn, pm.ProviderName
       HAVING pm.ProviderName IS NOT NULL
   )

   UPDATE pm
   SET pm.ProviderName = upm.ReplacementProviderName
   FROM employer_financial.PaymentMetaData pm
   INNER JOIN employer_financial.Payment payment
       ON payment.PaymentMetaDataId = pm.id
   INNER JOIN
       (
           SELECT p.UKprn, MAX(ap.ProviderName) as ReplacementProviderName
           FROM employer_financial.PaymentMetaData pm
           INNER JOIN employer_financial.Payment p
               ON p.PaymentMetaDataId = pm.id
           INNER JOIN ALLProviders ap
               ON ap.UKprn = p.UKprn
           GROUP BY p.UKprn, pm.ProviderName
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