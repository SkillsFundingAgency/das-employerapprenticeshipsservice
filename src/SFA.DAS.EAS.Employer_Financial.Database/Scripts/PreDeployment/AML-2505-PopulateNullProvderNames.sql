-- Find problem records
SELECT p.UKprn, pm.ProviderName FROM employer_financial.Payment p
INNER JOIN employer_financial.PaymentMetaData pm
    ON p.PaymentMetaDataId = pm.id
GROUP BY p.UKprn, pm.ProviderName


-- Get all the UkPrns and the provider name they should be into a temporary named result set
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

-- Join this result set to any that are missing provider names
SELECT p.UKprn, pm.ProviderName, MAX(ap.ProviderName) as ReplacementProviderName FROM employer_financial.Payment p
INNER JOIN employer_financial.PaymentMetaData pm
    ON p.PaymentMetaDataId = pm.id
INNER JOIN ALLProviders ap
    ON ap.UKprn = p.UKprn
GROUP BY p.UKprn, pm.ProviderName
HAVING pm.ProviderName IS NULL

BEGIN TRAN

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

    -- Find problem records
    SELECT p.UKprn, pm.ProviderName FROM employer_financial.Payment p
    INNER JOIN employer_financial.PaymentMetaData pm
        ON p.PaymentMetaDataId = pm.id
    GROUP BY p.UKprn, pm.ProviderName

COMMIT TRAN