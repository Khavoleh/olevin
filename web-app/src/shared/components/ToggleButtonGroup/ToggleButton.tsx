"use client";

import {
	ToggleButton as AriaToggleButton,
	type ToggleButtonProps as AriaToggleButtonProps,
	composeRenderProps,
} from "react-aria-components";

const STATE_CLASSES = {
	idle: "text-fg-muted",
	hovered: "bg-surface-hover text-fg-muted",
	selected: "bg-primary text-on-primary",
};

/** Segment of a `ToggleButtonGroup`. */
const ToggleButton = ({
	className,
	...props
}: Readonly<AriaToggleButtonProps>) => {
	return (
		<AriaToggleButton
			{...props}
			className={composeRenderProps(
				className,
				(customClasses, { isSelected, isHovered }) => {
					const colors =
						(isSelected && STATE_CLASSES.selected) ||
						(isHovered && STATE_CLASSES.hovered) ||
						STATE_CLASSES.idle;

					return `min-h-10 cursor-pointer rounded-[calc(var(--radius-md)-1px)] px-3 font-semibold text-sm transition-colors ${colors} ${customClasses ?? ""}`;
				},
			)}
		/>
	);
};

export default ToggleButton;
