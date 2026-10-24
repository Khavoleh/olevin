"use client";

import {
	Tooltip as AriaTooltip,
	type TooltipProps as AriaTooltipProps,
	OverlayArrow,
} from "react-aria-components";

interface TooltipProps extends Omit<AriaTooltipProps, "children"> {
	children: string;
}

/** Short dark hint with an arrow next to the element it describes; place it inside a `TooltipTrigger`. */
const Tooltip = ({
	children,
	offset = 8,
	...props
}: Readonly<TooltipProps>) => {
	return (
		<AriaTooltip
			{...props}
			offset={offset}
			className="group max-w-64 rounded-md bg-fg px-3 py-2 text-background text-sm shadow-overlay data-entering:animate-popover-in data-exiting:animate-popover-out"
		>
			<OverlayArrow>
				<svg
					aria-hidden
					width={8}
					height={8}
					viewBox="0 0 8 8"
					className="block fill-fg group-data-[placement=bottom]:rotate-180"
				>
					<path d="M0 0 L4 4 L8 0" />
				</svg>
			</OverlayArrow>
			{children}
		</AriaTooltip>
	);
};

export default Tooltip;
