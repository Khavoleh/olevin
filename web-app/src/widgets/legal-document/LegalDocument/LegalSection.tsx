import { Heading } from "@shared/components";
import type { ReactNode } from "react";

interface LegalSectionProps {
	title: string;
	children: ReactNode;
}

/** Numbered section of a legal document: the heading and its paragraphs and lists. */
const LegalSection = ({ title, children }: Readonly<LegalSectionProps>) => {
	return (
		<section className="flex flex-col gap-3">
			<Heading level={2}>{title}</Heading>
			{children}
		</section>
	);
};

export default LegalSection;
