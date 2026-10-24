"use client";

import {
	showErrorToast,
	ToggleButton,
	ToggleButtonGroup,
} from "@shared/components";
import { LANGUAGES_SHORT } from "@shared/constants";
import { getI18n } from "@shared/helpers";
import { useTransition } from "react";
import { LANGUAGE_SWITCHER_I18N } from "./language-switcher-i18n";
import { setLanguage } from "./set-language";

/** Switcher buttons: the short label on screen and the full name for screen readers. */
const LANGUAGES_CONFIG = [
	{ code: LANGUAGES_SHORT.UK, label: "UA", name: "Українська" },
	{ code: LANGUAGES_SHORT.EN, label: "EN", name: "English" },
];

interface LanguageSwitcherProps {
	language: string;
	selectedLanguage?: string;
	saveLanguage?: (language: string) => Promise<string | undefined>;
}

/** Toggle between the supported languages; the choice is saved by `saveLanguage` when given and remembered in a cookie. */
const LanguageSwitcher = ({
	language,
	selectedLanguage = language,
	saveLanguage,
}: Readonly<LanguageSwitcherProps>) => {
	const t = getI18n(language, LANGUAGE_SWITCHER_I18N);
	const [isPending, startTransition] = useTransition();

	const select = (selected: string) =>
		startTransition(async () => {
			const error = await saveLanguage?.(selected);

			if (error) showErrorToast(error);
			else await setLanguage(selected);
		});

	return (
		<ToggleButtonGroup
			aria-label={t("change_language")}
			selectionMode="single"
			disallowEmptySelection
			isDisabled={isPending}
			selectedKeys={[selectedLanguage]}
			onSelectionChange={(keys) => {
				const [selected] = keys;

				if (typeof selected === "string" && selected !== selectedLanguage) {
					select(selected);
				}
			}}
		>
			{LANGUAGES_CONFIG.map((item) => (
				<ToggleButton
					key={item.code}
					id={item.code}
					aria-label={item.name}
					lang={item.code}
				>
					{item.label}
				</ToggleButton>
			))}
		</ToggleButtonGroup>
	);
};

export default LanguageSwitcher;
