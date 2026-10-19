"use client";

import {
	Input as AriaInput,
	type InputProps as AriaInputProps,
	composeRenderProps,
} from "react-aria-components";

const STATE_CLASSES = {
	idle: "border-input",
	hovered: "border-input-hover",
	invalid: "border-input-invalid",
	disabled: "border-input-disabled text-fg-disabled",
};

/** Text input of a form field, bordered by its state. */
const Input = ({ className, ...props }: Readonly<AriaInputProps>) => {
	return (
		<AriaInput
			{...props}
			className={composeRenderProps(
				className,
				(customClasses, { isHovered, isInvalid, isDisabled }) => {
					const border =
						(isDisabled && STATE_CLASSES.disabled) ||
						(isInvalid && STATE_CLASSES.invalid) ||
						(isHovered && STATE_CLASSES.hovered) ||
						STATE_CLASSES.idle;

					return `min-h-11 w-full rounded-md border bg-surface px-3 text-fg transition-colors ${border} ${customClasses ?? ""}`;
				},
			)}
		/>
	);
};

export default Input;
