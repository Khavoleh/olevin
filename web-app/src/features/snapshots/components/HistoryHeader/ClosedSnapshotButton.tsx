"use client";

import { Tooltip } from "@shared/components";
import type { ReactNode } from "react";
import { Focusable, TooltipTrigger } from "react-aria-components";

interface ClosedSnapshotButtonProps {
	hint: string;
	children: ReactNode;
}

/** Disabled new snapshot button with a tooltip of when it opens. */
const ClosedSnapshotButton = ({
	hint,
	children,
}: Readonly<ClosedSnapshotButtonProps>) => (
	<TooltipTrigger delay={300}>
		<Focusable>
			<button
				type="button"
				aria-disabled="true"
				className="inline-flex min-h-11 cursor-not-allowed items-center justify-center gap-2 rounded-md bg-primary px-4.5 font-semibold text-on-primary opacity-45"
			>
				{children}
			</button>
		</Focusable>
		<Tooltip>{hint}</Tooltip>
	</TooltipTrigger>
);

export default ClosedSnapshotButton;
