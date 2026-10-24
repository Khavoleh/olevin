import { Card, Heading, Text } from "@shared/components";
import type { ReactNode } from "react";

interface TrendCardProps {
	title: string;
	caption: string;
	children: ReactNode;
}

/** Card of one trend chart with its title and caption. */
const TrendCard = ({ title, caption, children }: Readonly<TrendCardProps>) => (
	<Card className="grid content-start gap-3">
		<Heading level={2}>{title}</Heading>
		{children}
		<Text variant="hint">{caption}</Text>
	</Card>
);

export default TrendCard;
