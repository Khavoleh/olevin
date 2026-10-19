"use client";

import {
	Heading as AriaHeading,
	type HeadingProps as AriaHeadingProps,
} from "react-aria-components";

interface HeadingProps extends Omit<AriaHeadingProps, "level"> {
	level: 1 | 2 | 3;
}

const LEVEL_CLASSES = {
	1: "text-balance font-heading font-semibold text-2xl md:text-3xl",
	2: "font-semibold text-lg",
	3: "font-semibold text-base",
};

/** Heading of the app on the type scale of its level. */
const Heading = ({
	level,
	className = "",
	...props
}: Readonly<HeadingProps>) => {
	return (
		<AriaHeading
			{...props}
			level={level}
			className={`${LEVEL_CLASSES[level]} ${className}`}
		/>
	);
};

export default Heading;
