"use client";

import {
	ConfirmDialog,
	Radio,
	RadioGroup,
	showErrorToast,
	Text,
} from "@shared/components";
import { getI18n } from "@shared/helpers";
import { useState, useTransition } from "react";
import { saveBaseCurrency } from "../../actions/save-base-currency";
import { CurrencyCode } from "../../api/profile.schemas";
import { PROFILE_I18N } from "../../profile-i18n";
import SettingsButton from "../SettingsList/SettingsButton";

interface BaseCurrencyEditorProps {
	language: string;
	baseCurrency: CurrencyCode;
}

/** Row of the base currency that opens the choice of a new one with the warning about the recalculation. */
const BaseCurrencyEditor = ({
	language,
	baseCurrency,
}: Readonly<BaseCurrencyEditorProps>) => {
	const t = getI18n(language, PROFILE_I18N);
	const currencyNames = new Intl.DisplayNames(language, { type: "currency" });
	const [isOpen, setIsOpen] = useState(false);
	const [selected, setSelected] = useState(baseCurrency);
	const [isPending, startTransition] = useTransition();

	const open = () => {
		setSelected(baseCurrency);
		setIsOpen(true);
	};

	const confirm = () =>
		startTransition(async () => {
			const error = await saveBaseCurrency(language, selected);

			if (error) showErrorToast(error);

			setIsOpen(false);
		});

	return (
		<>
			<SettingsButton
				label={t("base_currency")}
				hint={t("base_currency_hint")}
				value={baseCurrency}
				onPress={open}
			/>
			<ConfirmDialog
				title={t("base_currency_title")}
				confirmLabel={t("base_currency_confirm")}
				cancelLabel={t("cancel")}
				isOpen={isOpen}
				isPending={isPending}
				isConfirmDisabled={selected === baseCurrency}
				onOpenChange={setIsOpen}
				onConfirm={confirm}
			>
				<RadioGroup
					label={t("base_currency_new")}
					labelVariant="field"
					value={selected}
					onChange={(value) => setSelected(value as CurrencyCode)}
					isDisabled={isPending}
				>
					{Object.values(CurrencyCode).map((code) => (
						<Radio key={code} value={code}>
							<span className="font-semibold">{code}</span>
							<span className="text-fg-muted">{currencyNames.of(code)}</span>
						</Radio>
					))}
				</RadioGroup>
				<Text variant="description">{t("base_currency_warning")}</Text>
			</ConfirmDialog>
		</>
	);
};

export default BaseCurrencyEditor;
