"use client";

import { getFormProps, useForm } from "@conform-to/react";
import { parseWithZod } from "@conform-to/zod/v4";
import { Text, useFormErrorToast } from "@shared/components";
import { getI18n } from "@shared/helpers";
import {
	EstimateFields,
	getEstimateRequest,
	HiddenFields,
} from "@widgets/snapshot-fields";
import { useActionState, useState } from "react";
import { saveEstimate } from "../actions/save-estimate";
import type {
	CurrencyCode,
	GetSnapshotMonthResponseEstimatedMonthResponse,
} from "../api/snapshots.schemas";
import { UPDATE_ESTIMATE_SCHEMA } from "../constants/update-estimate-schema";
import { getMoneyDraft } from "../helpers/get-snapshot-draft/get-snapshot-draft";
import { SNAPSHOT_FORM_I18N } from "../snapshot-form-i18n";
import { useRecalculationConfirm } from "../use-recalculation-confirm";
import FormActions from "./FormActions";
import RecalculationDialog from "./RecalculationDialog";

interface EditEstimateFormProps {
	language: string;
	monthName: string;
	estimate: GetSnapshotMonthResponseEstimatedMonthResponse;
	baseCurrency: CurrencyCode;
}

/** Form of the rough amounts of an estimate that asks to confirm that every snapshot is recalculated. */
const EditEstimateForm = ({
	language,
	monthName,
	estimate,
	baseCurrency,
}: Readonly<EditEstimateFormProps>) => {
	const t = getI18n(language, SNAPSHOT_FORM_I18N);
	const [draft, setDraft] = useState(() => ({
		expensesTotal: getMoneyDraft(estimate.expensesTotal),
		saved: getMoneyDraft(estimate.saved),
	}));
	const [lastResult, action, isPending] = useActionState(
		saveEstimate,
		undefined,
	);
	const confirmation = useRecalculationConfirm(
		estimate.laterSnapshots,
		lastResult,
	);
	const [form] = useForm({
		lastResult,
		onValidate: ({ formData }) =>
			parseWithZod(formData, { schema: UPDATE_ESTIMATE_SCHEMA }),
		onSubmit: confirmation.onSubmit,
	});

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
					{t("description_estimate").replace("{month}", monthName)}
				</Text>

				<input type="hidden" name="language" value={language} />
				<HiddenFields
					value={{
						...getEstimateRequest(draft, baseCurrency),
						id: estimate.id,
					}}
				/>

				<EstimateFields
					language={language}
					title={`${monthName.charAt(0).toUpperCase()}${monthName.slice(1)}`}
					value={draft}
					baseCurrency={baseCurrency}
					onChange={setDraft}
				/>

				<FormActions
					language={language}
					submitLabel={t("save")}
					isPending={isPending}
				/>
			</form>

			<RecalculationDialog
				language={language}
				laterSnapshots={estimate.laterSnapshots}
				isOpen={confirmation.isOpen}
				isPending={isPending}
				onOpenChange={confirmation.setIsOpen}
				onConfirm={confirmation.confirm}
			/>
		</>
	);
};

export default EditEstimateForm;
