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
				(customClasses, { isSelected, isHovered, isFocusVisible }) => {
					const colors =
						(isSelected && STATE_CLASSES.selected) ||
						(isHovered && STATE_CLASSES.hovered) ||
						STATE_CLASSES.idle;
					const focus = isFocusVisible ? "ring-2 ring-ring" : "";

					return `min-h-10 cursor-pointer rounded-sm px-3 font-semibold text-sm outline-none transition-colors ${colors} ${focus} ${customClasses ?? ""}`;
				},
			)}
		/>
	);
};

export default ToggleButton;
