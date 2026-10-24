import { Card, Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { Suspense } from "react";
import { COMPOSITION_CARD_I18N } from "../../i18n/composition-card-i18n";
import CompositionItems from "./CompositionItems";

interface CompositionCardProps {
	language: string;
	className?: string;
}

const SKELETON_ITEMS = [1, 2, 3];

/** Card of what the index is made of: cash flow, financial buffer and savings regularity. */
const CompositionCard = ({
	language,
	className = "",
}: Readonly<CompositionCardProps>) => {
	const t = getI18n(language, COMPOSITION_CARD_I18N);

	return (
		<Card className={`grid content-start gap-3 ${className}`}>
			<Heading level={2}>{t("title")}</Heading>
			<div aria-live="polite" className="grid gap-4">
				<Suspense
					fallback={SKELETON_ITEMS.map((item) => (
						<div
							key={item}
							aria-hidden
							className="h-20 rounded-lg bg-skeleton"
						/>
					))}
				>
					<CompositionItems language={language} />
				</Suspense>
			</div>
		</Card>
	);
};

export default CompositionCard;
