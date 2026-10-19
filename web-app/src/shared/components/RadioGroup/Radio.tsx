"use client";

import type { ReactNode } from "react";
import {
	RadioButton,
	RadioField,
	type RadioFieldProps,
} from "react-aria-components";

interface RadioProps extends Omit<RadioFieldProps, "className" | "children"> {
	children: ReactNode;
}

const STATE_CLASSES = {
	idle: "border-input bg-surface",
	hovered: "border-input-hover bg-surface",
	selected: "border-primary bg-primary-soft",
};

/** Option of a radio group shown as a bordered row with the radio mark. */
const Radio = ({ children, ...props }: Readonly<RadioProps>) => {
	return (
		<RadioField {...props}>
			<RadioButton
				className={({ isSelected, isHovered, isDisabled }) => {
					const colors =
						(isSelected && STATE_CLASSES.selected) ||
						(isHovered && STATE_CLASSES.hovered) ||
						STATE_CLASSES.idle;
					const disabled = isDisabled
						? "cursor-default opacity-45"
						: "cursor-pointer";

					return `flex min-h-11 items-center gap-3 rounded-md border px-4 py-2.5 transition-colors ${colors} ${disabled}`;
				}}
			>
				{({ isSelected }) => (
					<>
						<span
							aria-hidden="true"
							className={`size-5 flex-none rounded-full transition-[border-width] ${
								isSelected ? "border-6 border-primary" : "border-2 border-input"
							}`}
						/>
						{children}
					</>
				)}
			</RadioButton>
		</RadioField>
	);
};

export default Radio;
