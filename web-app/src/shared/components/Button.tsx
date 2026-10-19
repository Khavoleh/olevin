"use client";

import {
	Button as AriaButton,
	type ButtonProps as AriaButtonProps,
	composeRenderProps,
} from "react-aria-components";

interface ButtonProps extends AriaButtonProps {
	variant?: "primary" | "secondary";
}

const VARIANT_CLASSES = {
	primary: {
		base: "text-on-primary",
		idle: "bg-primary",
		hovered: "bg-primary-hover",
		pressed: "bg-primary-pressed",
	},
	secondary: {
		base: "border border-input text-fg",
		idle: "bg-surface",
		hovered: "bg-surface-hover",
		pressed: "bg-surface-sunken",
	},
};

/** Accessible button of the app in the primary or secondary style. */
const Button = ({
	variant = "primary",
	className,
	...props
}: Readonly<ButtonProps>) => {
	return (
		<AriaButton
			{...props}
			className={composeRenderProps(
				className,
				(
					customClasses,
					{ isHovered, isPressed, isFocusVisible, isDisabled },
				) => {
					const { base, idle, hovered, pressed } = VARIANT_CLASSES[variant];
					const background =
						(isPressed && pressed) || (isHovered && hovered) || idle;
					const focus = isFocusVisible ? "ring-2 ring-ring ring-offset-2" : "";
					const disabled = isDisabled
						? "cursor-default opacity-45"
						: "cursor-pointer";

					return `inline-flex min-h-11 items-center justify-center rounded-md px-4.5 font-semibold outline-none transition-colors ${base} ${background} ${focus} ${disabled} ${customClasses}`;
				},
			)}
		/>
	);
};

export default Button;
