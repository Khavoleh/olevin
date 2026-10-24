import { Text } from "@shared/components";
import type { ReactNode } from "react";

interface SettingsGroupProps {
	title: string;
	children: ReactNode;
}

/** Titled list of settings; on a wide page the title moves to the left of the list. */
const SettingsGroup = ({ title, children }: Readonly<SettingsGroupProps>) => (
	<section className="grid @profile:grid-cols-[12.5rem_minmax(0,1fr)] @profile:items-start @profile:gap-6 gap-2">
		<Text variant="eyebrow" elementType="h2" className="@profile:pt-4">
			{title}
		</Text>
		<div className="divide-y divide-border rounded-lg border border-border bg-surface shadow-card [&>:first-child]:rounded-t-[calc(var(--radius-lg)-1px)] [&>:last-child]:rounded-b-[calc(var(--radius-lg)-1px)]">
			{children}
		</div>
	</section>
);

export default SettingsGroup;
