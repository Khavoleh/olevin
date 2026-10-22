import { Heading } from "@shared/components";
import { getI18n } from "@shared/helpers";
import type { ReactNode } from "react";
import { formatIndex } from "../../helpers/format-index/format-index";
import { ADVICE_CARD_I18N } from "../../i18n/advice-card-i18n";

interface AdviceRowProps {
	rank: number;
	title: string;
	gain: number;
	language: string;
	children: ReactNode;
}

/** One advice in the list: its place, what to do, the gain of the index and the details. */
const AdviceRow = ({
	rank,
	title,
	gain,
	language,
	children,
}: Readonly<AdviceRowProps>) => {
	const t = getI18n(language, ADVICE_CARD_I18N);

	return (
		<li className="grid grid-cols-[auto_1fr] gap-3 border-border border-t pt-3 first:border-t-0 first:pt-0">
			<span
				aria-hidden
				className="flex size-7 items-center justify-center rounded-full bg-primary-soft font-semibold text-primary text-sm tabular-nums"
			>
				{rank}
			</span>
			<div className="grid gap-1.5">
				<div className="flex flex-wrap items-start justify-between gap-x-3 gap-y-1">
					<Heading level={3}>{title}</Heading>
					<span className="flex h-6.5 shrink-0 items-center rounded-full bg-success-soft px-2.75 font-semibold text-positive text-sm tabular-nums">
						+{formatIndex(gain, language)} {t("gain")}
					</span>
				</div>
				<div className="grid gap-1">{children}</div>
			</div>
		</li>
	);
};

export default AdviceRow;
