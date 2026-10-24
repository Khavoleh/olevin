"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { Text, useFormErrorToast } from "@shared/components";
import { formatSnapshotMonth, getI18n } from "@shared/helpers";
import SnapshotFields, {
	createSnapshotDraft,
	getSnapshotRequest,
	HiddenFields,
} from "@widgets/snapshot-fields";
import { useActionState, useState } from "react";
import { saveNewSnapshot } from "../../actions/save-new-snapshot";
import type {
	CurrencyCode,
	OpenMonthResponse,
} from "../../api/snapshots.schemas";
import { CreateSnapshotBody } from "../../api/snapshots.zod";
import { SNAPSHOT_FORM_I18N } from "../../snapshot-form-i18n";
import FormActions from "../FormActions";
import MonthField from "./MonthField";

interface NewSnapshotFormProps {
	language: string;
	months: OpenMonthResponse[];
	baseCurrency: CurrencyCode;
}

/** Form of a new snapshot for one of the open months, the latest one at first. */
const NewSnapshotForm = ({
	language,
	months,
	baseCurrency,
}: Readonly<NewSnapshotFormProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);
	const [draft, setDraft] = useState(createSnapshotDraft);
	const [monthIndex, setMonthIndex] = useState(0);
	const [lastResult, action, isPending] = useActionState(
		saveNewSnapshot,
		undefined,
	);
	const [form, fields] = useForm({
		lastResult,
		onValidate: ({ formData }) =>
			parseWithZod(formData, { schema: CreateSnapshotBody }),
		shouldRevalidate: "onInput",
	});
	const { year, month } = months[monthIndex];
	const request = getSnapshotRequest(draft, baseCurrency);

	useFormErrorToast(lastResult);

	return (
		<form {...getFormProps(form)} action={action} className="grid gap-4">
			<Text variant="description">
				{t("description_new").replace(
					"{month}",
					formatSnapshotMonth(year, month, language),
				)}
			</Text>

			<input type="hidden" name="language" value={language} />
			<HiddenFields
				value={{ ...request, selfRating: undefined, year, month }}
			/>

			<MonthField
				language={language}
				months={months}
				value={monthIndex}
				onChange={setMonthIndex}
			/>

			<SnapshotFields
				language={language}
				value={draft}
				baseCurrency={baseCurrency}
				selfRatingName={fields.selfRating.name}
				isSelfRatingInvalid={!!fields.selfRating.errors}
				onChange={(value) => setDraft((current) => ({ ...current, ...value }))}
			/>

			<FormActions
				language={language}
				submitLabel={t("save_new")}
				isPending={isPending}
			/>
		</form>
	);
};

export default NewSnapshotForm;
