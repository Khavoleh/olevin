"use client";

import {
	Label as AriaLabel,
	type LabelProps as AriaLabelProps,
} from "react-aria-components";

interface LabelProps extends AriaLabelProps {
	variant?: "field" | "group";
}

const VARIANT_CLASSES = {
	field: "font-semibold text-sm",
	group: "font-semibold text-base",
};

/** Label of a form field, or the larger title of a group of options. */
const Label = ({
	variant = "field",
	className = "",
	...props
}: Readonly<LabelProps>) => {
	return (
		<AriaLabel
			{...props}
			className={`${VARIANT_CLASSES[variant]} ${className}`}
		/>
	);
};

export default Label;
