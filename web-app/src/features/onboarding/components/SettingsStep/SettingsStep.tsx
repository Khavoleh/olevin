"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import {
	Checkbox,
	FormBlock,
	Label,
	NumberField,
	Text,
	useFormErrorToast,
} from "@shared/components";
import { getI18n } from "@shared/helpers";
import LanguageSwitcher from "@widgets/language-switcher";
import TimezoneField from "@widgets/timezone-field";
import { useActionState, useEffect } from "react";
import { configureSettings } from "../../actions/configure-settings";
import { SETTINGS_SCHEMA } from "../../constants/settings-schema";
import { SETTINGS_STEP_I18N } from "../../i18n/settings-step-i18n";
import { useOnboardingStore } from "../../onboarding-store";
import SubmitButton from "../SubmitButton";
import CurrencyField from "./CurrencyField";

interface SettingsStepProps {
	language: string;
}

/** Step with the language, base currency, snapshot day, time zone and research consent that saves the settings. */
const SettingsStep = ({ language }: Readonly<SettingsStepProps>) => {
	const t = getI18n(language, SETTINGS_STEP_I18N);
	const snapshotDay = useOnboardingStore((state) => state.snapshotDay);
	const setSnapshotDay = useOnboardingStore((state) => state.setSnapshotDay);
	const baseCurrency = useOnboardingStore((state) => state.baseCurrency);
	const timezone = useOnboardingStore((state) => state.timezone);
	const setTimezone = useOnboardingStore((state) => state.setTimezone);
	const researchConsent = useOnboardingStore((state) => state.researchConsent);
	const setResearchConsent = useOnboardingStore(
		(state) => state.setResearchConsent,
	);
	const [lastResult, action, isPending] = useActionState(
		configureSettings,
		undefined,
	);
	const [form, fields] = useForm({
		lastResult,
		onValidate: ({ formData }) =>
			parseWithZod(formData, { schema: SETTINGS_SCHEMA }),
		shouldValidate: "onBlur",
		shouldRevalidate: "onInput",
	});

	const isComplete = SETTINGS_SCHEMA.safeParse({
		baseCurrency,
		locale: language.toUpperCase(),
		snapshotDay,
		timezone,
		researchConsent,
	}).success;

	useFormErrorToast(lastResult);

	useEffect(() => {
		void useOnboardingStore.persist.rehydrate()?.then(() => {
			const { timezone, setTimezone } = useOnboardingStore.getState();

			if (!timezone) {
				setTimezone(Intl.DateTimeFormat().resolvedOptions().timeZone);
			}
		});
	}, []);

	return (
		<form {...getFormProps(form)} action={action} className="grid gap-4">
			<Text variant="description">{t("description")}</Text>

			<input
				type="hidden"
				name={fields.locale.name}
				value={language.toUpperCase()}
			/>

			<FormBlock>
				<Label variant="group" elementType="span">
					{t("language")}
				</Label>
				<div>
					<LanguageSwitcher language={language} />
				</div>
				<Text variant="hint">{t("language_hint")}</Text>
			</FormBlock>

			<FormBlock>
				<CurrencyField
					language={language}
					name={fields.baseCurrency.name}
					isInvalid={!!fields.baseCurrency.errors}
				/>
			</FormBlock>

			<FormBlock>
				<NumberField
					label={t("snapshot_day")}
					description={t("snapshot_day_hint")}
					errorMessage={t("snapshot_day_error")}
					labelVariant="group"
					inputClassName="w-32"
					name={fields.snapshotDay.name}
					value={snapshotDay ?? Number.NaN}
					onChange={setSnapshotDay}
					minValue={1}
					maxValue={31}
					formatOptions={{ maximumFractionDigits: 0, useGrouping: false }}
					isInvalid={!!fields.snapshotDay.errors}
					isRequired
					validationBehavior="aria"
				/>
			</FormBlock>

			<FormBlock>
				<TimezoneField
					language={language}
					value={timezone}
					onChange={setTimezone}
					name={fields.timezone.name}
					labelVariant="group"
					isInvalid={!!fields.timezone.errors}
				/>
			</FormBlock>

			<FormBlock>
				<Label variant="group" elementType="span">
					{t("research_consent")}
				</Label>
				<Checkbox
					name={fields.researchConsent.name}
					isSelected={researchConsent}
					onChange={setResearchConsent}
					description={t("research_consent_hint")}
				>
					{t("research_consent_label")}
				</Checkbox>
			</FormBlock>

			<SubmitButton
				language={language}
				isComplete={isComplete}
				isPending={isPending}
			>
				{t("continue")}
			</SubmitButton>
		</form>
	);
};

export default SettingsStep;
