"use client";

import { CaretRightIcon } from "@phosphor-icons/react";
import { Button, type ButtonProps } from "react-aria-components";
import SettingsLabel from "./SettingsLabel";
import { SETTINGS_ROW_CLASSES } from "./settings-row-classes";

interface SettingsButtonProps
	extends Omit<ButtonProps, "children" | "className"> {
	label: string;
	hint?: string;
	value?: string;
	tone?: "default" | "danger";
}

/** Whole-row button of a setting that opens its editor or runs its action. */
const SettingsButton = ({
	label,
	hint,
	value,
	tone,
	...props
}: Readonly<SettingsButtonProps>) => (
	<Button
		{...props}
		className={({ isHovered, isPressed }) => {
			const background =
				(isPressed && "bg-surface-sunken") ||
				(isHovered && "bg-surface-hover") ||
				"bg-transparent";

			return `${SETTINGS_ROW_CLASSES} cursor-pointer transition-colors ${background}`;
		}}
	>
		<SettingsLabel label={label} hint={hint} tone={tone} />
		<span className="flex flex-none items-center gap-1 whitespace-nowrap text-fg-muted text-sm">
			{value}
			<CaretRightIcon size={16} aria-hidden />
		</span>
	</Button>
);

export default SettingsButton;
