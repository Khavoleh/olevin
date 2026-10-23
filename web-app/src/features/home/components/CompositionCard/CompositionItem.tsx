import { Heading } from "@shared/components";
import type { ReactNode } from "react";

interface CompositionItemProps {
	title: string;
	summary: ReactNode;
	children: ReactNode;
}

/** One part of the index: its name, the value in short and the details. */
const CompositionItem = ({
	title,
	summary,
	children,
}: Readonly<CompositionItemProps>) => {
	return (
		<div className="grid content-start gap-1.5">
			<div className="flex items-baseline justify-between gap-2">
				<Heading level={3}>{title}</Heading>
				<span className="whitespace-nowrap text-fg-muted text-sm tabular-nums">
					{summary}
				</span>
			</div>
			{children}
		</div>
	);
};

export default CompositionItem;
