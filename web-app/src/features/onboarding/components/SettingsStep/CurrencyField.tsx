"use client";

import { Radio, RadioGroup } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { CurrencyCode } from "../../api/onboarding.schemas";
import { SETTINGS_STEP_I18N } from "../../i18n/settings-step-i18n";
import { useOnboardingStore } from "../../onboarding-store";

interface CurrencyFieldProps {
	language: string;
	name: string;
	isInvalid: boolean;
}

/** Choice of the base currency among the supported ones. */
const CurrencyField = ({
	language,
	name,
	isInvalid,
}: Readonly<CurrencyFieldProps>) => {
	const t = getI18n(language, SETTINGS_STEP_I18N);
	const baseCurrency = useOnboardingStore((state) => state.baseCurrency);
	const setBaseCurrency = useOnboardingStore((state) => state.setBaseCurrency);
	const currencyNames = new Intl.DisplayNames(language, { type: "currency" });

	return (
		<RadioGroup
			label={t("base_currency")}
			description={t("base_currency_hint")}
			errorMessage={t("base_currency_error")}
			name={name}
			value={baseCurrency ?? null}
			onChange={(value) => setBaseCurrency(value as CurrencyCode)}
			isInvalid={isInvalid}
			isRequired
			validationBehavior="aria"
		>
			{Object.values(CurrencyCode).map((code) => (
				<Radio key={code} value={code}>
					<span className="font-semibold">{code}</span>
					<span className="text-fg-muted">{currencyNames.of(code)}</span>
				</Radio>
			))}
		</RadioGroup>
	);
};

export default CurrencyField;
