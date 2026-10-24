"use client";

import { type ReactNode, useId } from "react";
import { Focusable, TooltipTrigger } from "react-aria-components";
import Tooltip from "./Tooltip";

interface DisabledButtonProps {
	/** Why the button is disabled. */
	hint: string;
	/** Whether the hint shows as a tooltip; otherwise only screen readers announce it. */
	hasTooltip?: boolean;
	className: string;
	children: ReactNode;
	"aria-label"?: string;
}

/** Disabled button that stays focusable, so it can tell why it is disabled. */
const DisabledButton = ({
	hint,
	hasTooltip = true,
	className,
	children,
	...props
}: Readonly<DisabledButtonProps>) => {
	const hintId = useId();

	if (!hasTooltip) {
		return (
			<>
				<button
					{...props}
					type="button"
					aria-disabled="true"
					aria-describedby={hintId}
					className={`cursor-not-allowed ${className}`}
				>
					{children}
				</button>
				<span id={hintId} hidden>
					{hint}
				</span>
			</>
		);
	}

	return (
		<TooltipTrigger delay={300}>
			<Focusable>
				<button
					{...props}
					type="button"
					aria-disabled="true"
					className={`cursor-not-allowed ${className}`}
				>
					{children}
				</button>
			</Focusable>
			<Tooltip>{hint}</Tooltip>
		</TooltipTrigger>
	);
};

export default DisabledButton;
