"use client";

import { Radio, RadioGroup, Text } from "@shared/components";
import { getI18n } from "@shared/helpers";
import { SNAPSHOT_FIELDS_I18N } from "../i18n/snapshot-fields-i18n";

const RATINGS = [1, 2, 3, 4, 5];

interface SelfRatingFieldProps {
	language: string;
	name: string;
	value?: number;
	isInvalid: boolean;
	onChange: (value: number) => void;
}

/** Rating of how calm the user feels about money, from 1 to 5. */
const SelfRatingField = ({
	language,
	name,
	value,
	isInvalid,
	onChange,
}: Readonly<SelfRatingFieldProps>) => {
	const t = getI18n(language, SNAPSHOT_FIELDS_I18N);
	const labels: Record<number, string> = {
		1: `1, ${t("self_rating_low")}`,
		5: `5, ${t("self_rating_high")}`,
	};

	return (
		<>
			<RadioGroup
				label={t("self_rating")}
				description={t("self_rating_hint")}
				errorMessage={t("self_rating_error")}
				name={name}
				value={value ? String(value) : null}
				onChange={(rating) => onChange(Number(rating))}
				isInvalid={isInvalid}
				isRequired
				validationBehavior="aria"
			>
				<div className="grid grid-cols-5 gap-1.5">
					{RATINGS.map((rating) => (
						<Radio
							key={rating}
							value={String(rating)}
							variant="tile"
							aria-label={labels[rating]}
						>
							{rating}
						</Radio>
					))}
				</div>
			</RadioGroup>
			<div aria-hidden="true" className="flex justify-between gap-2.5">
				<Text variant="hint" elementType="span">
					{t("self_rating_low")}
				</Text>
				<Text variant="hint" elementType="span">
					{t("self_rating_high")}
				</Text>
			</div>
		</>
	);
};

export default SelfRatingField;
