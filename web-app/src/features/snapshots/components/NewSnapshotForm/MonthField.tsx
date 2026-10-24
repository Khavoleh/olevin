"use client";

import { FormBlock, Select, Text } from "@shared/components";
import { formatSnapshotMonth, getI18n } from "@shared/helpers";
import type { OpenMonthResponse } from "../../api/snapshots.schemas";
import { SNAPSHOT_FORM_I18N } from "../../snapshot-form-i18n";

interface MonthFieldProps {
	language: string;
	months: OpenMonthResponse[];
	value: number;
	onChange: (value: number) => void;
}

/** Choice of the open month by its position in the list, shown only when there are several. */
const MonthField = ({
	language,
	months,
	value,
	onChange,
}: Readonly<MonthFieldProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);

	if (months.length < 2) return null;

	const options = months.map(({ year, month }, index) => {
		const name = formatSnapshotMonth(year, month, language);

		return {
			id: index,
			name: `${name.charAt(0).toUpperCase()}${name.slice(1)}`,
		};
	});

	return (
		<FormBlock>
			<Select
				label={t("month")}
				options={options}
				value={value}
				onChange={(index) => onChange(Number(index))}
			/>
			<Text variant="hint">{t("month_hint")}</Text>
		</FormBlock>
	);
};

export default MonthField;
