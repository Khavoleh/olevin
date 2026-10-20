"use client";

import type { ReactNode } from "react";
import {
	RadioButton,
	RadioField,
	type RadioFieldProps,
} from "react-aria-components";

interface RadioProps extends Omit<RadioFieldProps, "className" | "children"> {
	variant?: "row" | "tile" | "text-tile";
	children: ReactNode;
}

const VARIANT_CLASSES = {
	row: {
		base: "flex min-h-11 items-center gap-3 px-4 py-2.5",
		idle: "border-input bg-surface",
		hovered: "border-input-hover bg-surface",
		selected: "border-primary bg-primary-soft",
	},
	tile: {
		base: "flex min-h-12 items-center justify-center font-semibold text-lg tabular-nums",
		idle: "border-input bg-surface",
		hovered: "border-input-hover bg-surface-hover",
		selected: "border-primary bg-primary text-on-primary",
	},
	"text-tile": {
		base: "flex min-h-11 items-center justify-center px-2 py-2 text-center font-semibold text-sm leading-tight",
		idle: "border-input bg-surface",
		hovered: "border-input-hover bg-surface-hover",
		selected: "border-primary bg-primary text-on-primary",
	},
};

/** Option of a radio group: a bordered row with the radio mark, or a tile with a number or a word filled when selected. */
const Radio = ({
	variant = "row",
	children,
	...props
}: Readonly<RadioProps>) => {
	const { base, idle, hovered, selected } = VARIANT_CLASSES[variant];

	return (
		<RadioField {...props}>
			<RadioButton
				className={({ isSelected, isHovered, isDisabled }) => {
					const colors =
						(isSelected && selected) || (isHovered && hovered) || idle;
					const disabled = isDisabled
						? "cursor-default opacity-45"
						: "cursor-pointer";

					return `${base} rounded-md border transition-colors ${colors} ${disabled}`;
				}}
			>
				{({ isSelected }) => (
					<>
						{variant === "row" && (
							<span
								aria-hidden="true"
								className={`size-5 flex-none rounded-full transition-[border-width] ${
									isSelected
										? "border-6 border-primary"
										: "border-2 border-input"
								}`}
							/>
						)}
						{children}
					</>
				)}
			</RadioButton>
		</RadioField>
	);
};

export default Radio;
