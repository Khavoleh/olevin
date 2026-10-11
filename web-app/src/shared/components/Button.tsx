"use client";

import {
	Button as AriaButton,
	type ButtonProps as AriaButtonProps,
	composeRenderProps,
} from "react-aria-components";

interface ButtonProps extends AriaButtonProps {
	variant?: "primary" | "secondary";
}

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
					const isPrimary = variant === "primary";
					const colors = isPrimary
						? `text-on-primary ${isPressed ? "bg-primary-pressed" : isHovered ? "bg-primary-hover" : "bg-primary"}`
						: `border border-input text-fg ${isPressed ? "bg-surface-sunken" : isHovered ? "bg-surface-hover" : "bg-surface"}`;
					const focus = isFocusVisible ? "ring-2 ring-ring ring-offset-2" : "";
					const disabled = isDisabled
						? "cursor-default opacity-50"
						: "cursor-pointer";

					return `inline-flex min-h-11 items-center justify-center rounded-md px-4 font-semibold outline-none transition-colors ${colors} ${focus} ${disabled} ${customClasses}`;
				},
			)}
		/>
	);
};

export default Button;
