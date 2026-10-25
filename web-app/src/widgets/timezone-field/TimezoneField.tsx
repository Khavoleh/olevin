"use client";

import { ComboBox, ComboBoxItem } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useMemo, useSyncExternalStore } from "react";
import {
	getTimezoneOption,
	getTimezoneOptions,
} from "./helpers/get-timezone-options";
import { TIMEZONE_FIELD_I18N } from "./timezone-field-i18n";

const subscribeToClient = () => () => {};
const getIsClient = () => true;
const getIsServer = () => false;

interface TimezoneFieldProps {
	language: string;
	value: string;
	onChange: (timezone: string) => void;
	name?: string;
	labelVariant?: "field" | "group";
	isInvalid?: boolean;
	isDisabled?: boolean;
}

/** Searchable choice of the IANA time zone. */
const TimezoneField = ({
	language,
	value,
	onChange,
	name,
	labelVariant,
	isInvalid,
	isDisabled,
}: Readonly<TimezoneFieldProps>) => {
	const t = getI18n(language, TIMEZONE_FIELD_I18N);
	const isClient = useSyncExternalStore(
		subscribeToClient,
		getIsClient,
		getIsServer,
	);
	const zones = useMemo(
		() => (isClient ? getTimezoneOptions(language) : []),
		[isClient, language],
	);
	const isListed = !value || zones.some((zone) => zone.id === value);
	const options = isListed
		? zones
		: [...zones, getTimezoneOption(language, value)];

	return (
		<ComboBox
			label={t("timezone")}
			buttonLabel={t("show_timezones")}
			description={t("timezone_hint")}
			errorMessage={t("timezone_error")}
			labelVariant={labelVariant}
			name={name}
			formValue="key"
			defaultItems={options}
			value={value || null}
			onChange={(key) => {
				if (key) onChange(String(key));
			}}
			isInvalid={isInvalid}
			isDisabled={isDisabled}
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
