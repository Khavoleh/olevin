"use client";

import {
	ToggleButtonGroup as AriaToggleButtonGroup,
	type ToggleButtonGroupProps as AriaToggleButtonGroupProps,
	composeRenderProps,
} from "react-aria-components";

/** Segmented control of `ToggleButton`s in a bordered frame. */
const ToggleButtonGroup = ({
	className,
	...props
}: Readonly<AriaToggleButtonGroupProps>) => {
	return (
		<AriaToggleButtonGroup
			{...props}
			className={composeRenderProps(
				className,
				(customClasses) =>
					`inline-flex rounded-md border border-input bg-surface ${customClasses ?? ""}`,
			)}
		/>
	);
};

export default ToggleButtonGroup;
