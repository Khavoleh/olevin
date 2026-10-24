"use client";

import { useId } from "react";
import {
	SwitchButton,
	SwitchField,
	type SwitchFieldProps,
} from "react-aria-components";
import SettingsLabel from "./SettingsLabel";
import { SETTINGS_ROW_CLASSES } from "./settings-row-classes";

interface SettingsSwitchProps
	extends Omit<SwitchFieldProps, "children" | "className"> {
	label: string;
	hint?: string;
}

/** Setting that is on or off; the whole row toggles it. */
const SettingsSwitch = ({
	label,
	hint,
	...props
}: Readonly<SettingsSwitchProps>) => {
	const hintId = useId();

	return (
		<SwitchField {...props} aria-describedby={hint ? hintId : undefined}>
			<SwitchButton
				className={`${SETTINGS_ROW_CLASSES} cursor-pointer rounded-[inherit]`}
			>
				{({ isSelected }) => (
					<>
						<SettingsLabel label={label} hint={hint} hintId={hintId} />
						<span
							aria-hidden
							className={`relative h-7 w-11.5 flex-none rounded-full transition-colors ${isSelected ? "bg-primary" : "bg-input"}`}
						>
							<span
								className={`absolute top-0.75 left-0.75 size-5.5 rounded-full bg-surface shadow-card transition-transform ${isSelected ? "translate-x-4.5" : ""}`}
							/>
						</span>
					</>
				)}
			</SwitchButton>
		</SwitchField>
	);
};

export default SettingsSwitch;
