"use client";

import {
	Text as AriaText,
	type TextProps as AriaTextProps,
} from "react-aria-components";

interface TextProps extends AriaTextProps {
	variant?: "body" | "description" | "hint" | "eyebrow";
}

const VARIANT_CLASSES = {
	body: "text-base text-fg",
	description: "text-base text-fg-muted",
	hint: "text-fg-muted text-sm",
	eyebrow: "font-semibold text-fg-muted text-xs uppercase tracking-label",
};

/** Paragraph of the app: body text, a muted description, a small hint or an uppercase eyebrow above a heading. */
const Text = ({
	variant = "body",
	elementType = "p",
	className = "",
	...props
}: Readonly<TextProps>) => {
	return (
		<AriaText
			{...props}
			elementType={elementType}
			className={`${VARIANT_CLASSES[variant]} ${className}`}
		/>
	);
};

export default Text;
