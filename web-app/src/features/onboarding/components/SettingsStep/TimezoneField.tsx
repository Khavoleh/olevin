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

const getTimezoneName = (
	language: string,
	timeZone: string,
	timeZoneName: "longOffset" | "longGeneric",
) =>
	new Intl.DateTimeFormat(language, { timeZone, timeZoneName })
		.formatToParts()
		.find((part) => part.type === "timeZoneName")?.value ?? "";

const getTimezoneOption = (language: string, id: string) => {
	const offset = getTimezoneName("en", id, "longOffset");
	const name = getTimezoneName(language, id, "longGeneric");

	return {
		id,
		label: `${id.replaceAll("_", " ")} (${offset})`,
		description: name.startsWith("GMT") ? undefined : name,
	};
};

const getTimezoneOptions = (language: string) => {
	const zones = Intl.supportedValuesOf("timeZone").map(
		(zone) => RENAMED_TIMEZONES[zone] ?? zone,
	);

	return [...new Set(zones)]
		.sort()
		.map((zone) => getTimezoneOption(language, zone));
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
		() => (isClient ? getTimezoneOptions(language) : []),
		[isClient, language],
	);
	const isListed = !timezone || zones.some((zone) => zone.id === timezone);
	const options = isListed
		? zones
		: [...zones, getTimezoneOption(language, timezone)];

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
				<ComboBoxItem
					id={option.id}
					textValue={option.label}
					description={option.description}
				>
					{option.label}
				</ComboBoxItem>
			)}
		</ComboBox>
	);
};

export default TimezoneField;
