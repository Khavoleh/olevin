"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { Text, useFormErrorToast } from "@shared/components";
import { getI18n } from "@shared/helpers";
import SnapshotFields, {
	getSnapshotRequest,
	HiddenFields,
} from "@widgets/snapshot-fields";
import { useActionState, useState } from "react";
import { saveSnapshot } from "../actions/save-snapshot";
import type {
	CurrencyCode,
	GetSnapshotMonthResponseActualMonthResponse,
} from "../api/snapshots.schemas";
import { UPDATE_SNAPSHOT_SCHEMA } from "../constants/update-snapshot-schema";
import { getSnapshotDraft } from "../helpers/get-snapshot-draft/get-snapshot-draft";
import { SNAPSHOT_FORM_I18N } from "../snapshot-form-i18n";
import { useRecalculationConfirm } from "../use-recalculation-confirm";
import FormActions from "./FormActions";
import RecalculationDialog from "./RecalculationDialog";

interface EditSnapshotFormProps {
	language: string;
	monthName: string;
	snapshot: GetSnapshotMonthResponseActualMonthResponse;
	baseCurrency: CurrencyCode;
}

/** Form of the saved amounts of a snapshot that asks to confirm when later snapshots are recalculated. */
const EditSnapshotForm = ({
	language,
	monthName,
	snapshot,
	baseCurrency,
}: Readonly<EditSnapshotFormProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);
	const [draft, setDraft] = useState(() => getSnapshotDraft(snapshot));
	const [lastResult, action, isPending] = useActionState(
		saveSnapshot,
		undefined,
	);
	const confirmation = useRecalculationConfirm(
		snapshot.laterSnapshots,
		lastResult,
	);
	const [form, fields] = useForm({
		lastResult,
		onValidate: ({ formData }) =>
			parseWithZod(formData, { schema: UPDATE_SNAPSHOT_SCHEMA }),
		onSubmit: confirmation.onSubmit,
		shouldRevalidate: "onInput",
	});
	const request = getSnapshotRequest(draft, baseCurrency);

	useFormErrorToast(lastResult);

	return (
		<>
			<form
				ref={confirmation.formRef}
				{...getFormProps(form)}
				action={action}
				className="grid gap-4"
			>
				<Text variant="description">
					{t("description_edit").replace("{month}", monthName)}
				</Text>

				<input type="hidden" name="language" value={language} />
				<HiddenFields
					value={{ ...request, selfRating: undefined, id: snapshot.id }}
				/>

				<SnapshotFields
					language={language}
					value={draft}
					baseCurrency={baseCurrency}
					selfRatingName={fields.selfRating.name}
					isSelfRatingInvalid={!!fields.selfRating.errors}
					onChange={(value) =>
						setDraft((current) => ({ ...current, ...value }))
					}
				/>

				<FormActions
					language={language}
					submitLabel={t("save")}
					isPending={isPending}
				/>
			</form>

			<RecalculationDialog
				language={language}
				laterSnapshots={snapshot.laterSnapshots}
				isOpen={confirmation.isOpen}
				isPending={isPending}
				onOpenChange={confirmation.setIsOpen}
				onConfirm={confirmation.confirm}
			/>
		</>
	);
};

export default EditSnapshotForm;
