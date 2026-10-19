"use client";

import NextLink from "next/link";
import {
	Link as AriaLink,
	type LinkProps as AriaLinkProps,
	composeRenderProps,
} from "react-aria-components";

interface LinkProps extends Omit<AriaLinkProps, "href" | "render"> {
	href: string;
	variant?: "inline" | "subtle";
}

const VARIANT_CLASSES = {
	inline: {
		base: "font-semibold underline underline-offset-2",
		idle: "text-primary",
		hovered: "text-primary-hover",
	},
	subtle: {
		base: "",
		idle: "text-fg-muted",
		hovered: "text-fg underline underline-offset-2",
	},
};

/** Accessible link of the app with client-side navigation: inline in text or subtle in secondary text. */
const Link = ({
	variant = "inline",
	className,
	...props
}: Readonly<LinkProps>) => {
	return (
		<AriaLink
			{...props}
			render={(domProps) =>
				"href" in domProps ? <NextLink {...domProps} /> : <span {...domProps} />
			}
			className={composeRenderProps(
				className,
				(customClasses, { isHovered }) => {
					const { base, idle, hovered } = VARIANT_CLASSES[variant];
					const colors = isHovered ? hovered : idle;

					return `rounded-sm transition-colors ${base} ${colors} ${customClasses ?? ""}`;
				},
			)}
		/>
	);
};

export default Link;
