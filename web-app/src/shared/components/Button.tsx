"use client";

import {
	Button as AriaButton,
	type ButtonProps as AriaButtonProps,
	composeRenderProps,
} from "react-aria-components";

interface ButtonProps extends AriaButtonProps {
	variant?:
		| "primary"
		| "secondary"
		| "ghost"
		| "danger"
		| "ghost-danger"
		| "icon"
		| "icon-danger";
}

const VARIANT_CLASSES = {
	primary: {
		base: "px-4.5 text-on-primary",
		idle: "bg-primary",
		hovered: "bg-primary-hover",
		pressed: "bg-primary-pressed",
	},
	secondary: {
		base: "border border-input px-4.5 text-fg",
		idle: "bg-surface",
		hovered: "bg-surface-hover",
		pressed: "bg-surface-sunken",
	},
	ghost: {
		base: "gap-1.5 px-4.5 text-primary",
		idle: "bg-transparent",
		hovered: "bg-primary-soft",
		pressed: "bg-primary-soft",
	},
	danger: {
		base: "px-4.5 text-on-danger",
		idle: "bg-danger",
		hovered: "bg-danger-hover",
		pressed: "bg-danger-hover",
	},
	"ghost-danger": {
		base: "gap-1.5 px-4.5 text-danger",
		idle: "bg-transparent",
		hovered: "bg-danger-soft",
		pressed: "bg-danger-soft",
	},
	icon: {
		base: "w-11 flex-none text-fg-muted",
		idle: "bg-transparent",
		hovered: "bg-surface-hover",
		pressed: "bg-surface-sunken",
	},
	"icon-danger": {
		base: "w-11 flex-none text-danger",
		idle: "bg-transparent",
		hovered: "bg-danger-soft",
		pressed: "bg-danger-soft",
	},
};

/** Accessible button of the app in the primary, secondary, ghost, danger or ghost danger style, or a square icon button. */
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
				(customClasses, { isHovered, isPressed, isDisabled }) => {
					const { base, idle, hovered, pressed } = VARIANT_CLASSES[variant];
					const background =
						(isPressed && pressed) || (isHovered && hovered) || idle;
					const disabled = isDisabled
						? "cursor-default opacity-45"
						: "cursor-pointer";

					return `inline-flex min-h-11 items-center justify-center rounded-md font-semibold transition-colors ${base} ${background} ${disabled} ${customClasses}`;
				},
			)}
		/>
	);
};

export default Button;
