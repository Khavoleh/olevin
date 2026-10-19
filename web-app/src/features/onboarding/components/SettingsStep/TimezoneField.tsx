"use client";

import { ComboBox, ComboBoxItem } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useMemo, useSyncExternalStore } from "react";
import { SETTINGS_STEP_I18N } from "../../i18n/settings-step-i18n";
import { useOnboardingStore } from "../../onboarding-store";

const RENAMED_TIMEZONES: Record<string, string> = {
	"Europe/Kiev": "Europe/Kyiv",
};

const subscribeToClient = () => () => {};
const getIsClient = () => true;
const getIsServer = () => false;

const getOffset = (timeZone: string) =>
	new Intl.DateTimeFormat("en", { timeZone, timeZoneName: "longOffset" })
		.formatToParts()
		.find((part) => part.type === "timeZoneName")?.value ?? "";

const getTimezoneOption = (id: string) => ({
	id,
	label: `${id.replaceAll("_", " ")} (${getOffset(id)})`,
});

const getTimezoneOptions = () => {
	const zones = Intl.supportedValuesOf("timeZone").map(
		(zone) => RENAMED_TIMEZONES[zone] ?? zone,
	);

	return [...new Set(zones)].sort().map(getTimezoneOption);
};

interface TimezoneFieldProps {
	language: string;
	name: string;
	isInvalid: boolean;
}

/** Searchable choice of the IANA time zone, filled with the one of the device. */
const TimezoneField = ({
	language,
	name,
	isInvalid,
}: Readonly<TimezoneFieldProps>) => {
	const t = getI18n(language, SETTINGS_STEP_I18N);
	const timezone = useOnboardingStore((state) => state.timezone);
	const setTimezone = useOnboardingStore((state) => state.setTimezone);
	const isClient = useSyncExternalStore(
		subscribeToClient,
		getIsClient,
		getIsServer,
	);
	const zones = useMemo(
		() => (isClient ? getTimezoneOptions() : []),
		[isClient],
	);
	const isListed = !timezone || zones.some((zone) => zone.id === timezone);
	const options = isListed ? zones : [...zones, getTimezoneOption(timezone)];

	return (
		<ComboBox
			label={t("timezone")}
			buttonLabel={t("show_timezones")}
			description={t("timezone_hint")}
			errorMessage={t("timezone_error")}
			labelVariant="group"
			name={name}
			formValue="key"
			defaultItems={options}
			value={timezone || null}
			onChange={(key) => {
				if (key) setTimezone(String(key));
			}}
			isInvalid={isInvalid}
			isRequired
			validationBehavior="aria"
		>
			{(option) => (
				<ComboBoxItem id={option.id} textValue={option.label}>
					{option.label}
				</ComboBoxItem>
			)}
		</ComboBox>
	);
};

export default TimezoneField;
