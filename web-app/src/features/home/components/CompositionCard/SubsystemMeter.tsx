"use client";

import { Text } from "@shared/components";
import { formatIndex, getI18n } from "@shared/helpers";
import { Meter } from "react-aria-components";
import type { SubsystemTerm } from "../../api/home.schemas";
import { COMPOSITION_CARD_I18N } from "../../i18n/composition-card-i18n";
import CompositionItem from "./CompositionItem";

interface SubsystemMeterProps {
	title: string;
	value: number;
	term: SubsystemTerm;
	detail: string;
	language: string;
}

/** Subsystem of the index as a bar from 0 to 100 with its term and what it is calculated from. */
const SubsystemMeter = ({
	title,
	value,
	term,
	detail,
	language,
}: Readonly<SubsystemMeterProps>) => {
	const t = getI18n(language, COMPOSITION_CARD_I18N);
	const number = formatIndex(value, language);
	const scale = `${t("out_of")} · ${t(`term_${term}`)}`;

	return (
		<CompositionItem
			title={title}
			summary={
				<>
					<span className="font-semibold text-fg">{number}</span> {scale}
				</>
			}
		>
			<Meter
				value={value}
				aria-label={title}
				valueLabel={`${number} ${scale}`}
				className="h-2 overflow-hidden rounded-full bg-meter-track"
			>
				{({ percentage }) => (
					<div
						className="h-full rounded-full bg-meter"
						style={{ width: `${percentage}%` }}
					/>
				)}
			</Meter>
			<Text variant="hint">{detail}</Text>
		</CompositionItem>
	);
};

export default SubsystemMeter;
