"use client";

import {
	Button as AriaButton,
	type ButtonProps as AriaButtonProps,
	composeRenderProps,
} from "react-aria-components";

interface ButtonProps extends AriaButtonProps {
	variant?: "primary" | "secondary";
}

/** Colors of each button variant, including the hover and pressed states. */
const VARIANT_CLASSES: Record<NonNullable<ButtonProps["variant"]>, string> = {
	primary:
		"bg-primary text-on-primary data-hovered:bg-primary-hover data-pressed:bg-primary-pressed",
	secondary:
		"border border-border bg-surface text-text data-hovered:bg-muted data-pressed:bg-border",
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
				(customClasses) =>
					`inline-flex cursor-pointer items-center justify-center rounded-lg px-4 py-2 font-medium outline-none transition-colors data-disabled:cursor-default data-focus-visible:ring-2 data-focus-visible:ring-focus data-focus-visible:ring-offset-2 data-disabled:opacity-50 ${VARIANT_CLASSES[variant]} ${customClasses}`,
			)}
		/>
	);
};

export default Button;
