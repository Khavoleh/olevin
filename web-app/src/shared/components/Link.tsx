"use client";

import NextLink from "next/link";
import {
	Link as AriaLink,
	type LinkProps as AriaLinkProps,
	composeRenderProps,
} from "react-aria-components";

interface LinkProps extends Omit<AriaLinkProps, "href" | "render"> {
	href: string;
	variant?: "inline" | "subtle" | "button" | "secondary" | "action" | "icon";
	isDocument?: boolean;
}

const VARIANT_CLASSES = {
	inline: {
		base: "rounded-sm font-semibold underline underline-offset-2",
		idle: "text-primary",
		hovered: "text-primary-hover",
	},
	subtle: {
		base: "rounded-sm",
		idle: "text-fg-muted",
		hovered: "text-fg underline underline-offset-2",
	},
	button: {
		base: "inline-flex min-h-11 items-center justify-center gap-2 rounded-md px-4.5 font-semibold text-on-primary",
		idle: "bg-primary",
		hovered: "bg-primary-hover",
	},
	secondary: {
		base: "inline-flex min-h-11 items-center justify-center rounded-md border border-input px-4.5 font-semibold text-fg",
		idle: "bg-surface",
		hovered: "bg-surface-hover",
	},
	action: {
		base: "inline-flex min-h-11 items-center justify-center gap-1.5 rounded-md px-4.5 font-semibold",
		idle: "text-fg-muted",
		hovered: "bg-surface-hover text-fg",
	},
	icon: {
		base: "inline-flex size-11 flex-none items-center justify-center rounded-md",
		idle: "text-fg-muted",
		hovered: "bg-surface-hover text-fg",
	},
};

/** Accessible link of the app with client-side navigation: in text, in secondary text, or looking like a button; a document one (a route handler) loads without the client router. */
const Link = ({
	variant = "inline",
	isDocument = false,
	className,
	...props
}: Readonly<LinkProps>) => {
	return (
		<AriaLink
			{...props}
			render={(domProps) => {
				if (!("href" in domProps)) return <span {...domProps} />;

				return isDocument ? <a {...domProps} /> : <NextLink {...domProps} />;
			}}
			className={composeRenderProps(
				className,
				(customClasses, { isHovered }) => {
					const { base, idle, hovered } = VARIANT_CLASSES[variant];
					const colors = isHovered ? hovered : idle;

					return `transition-colors ${base} ${colors} ${customClasses ?? ""}`;
				},
			)}
		/>
	);
};

export default Link;
