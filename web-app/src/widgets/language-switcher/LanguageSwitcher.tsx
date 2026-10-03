"use client";

import { LANGUAGES_SHORT } from "@shared/constants";
import { getI18n } from "@shared/helpers";
import { ToggleButton, ToggleButtonGroup } from "react-aria-components";
import { LANGUAGE_SWITCHER_I18N } from "./language-switcher-i18n";
import { setLanguage } from "./set-language";

/** Switcher buttons: the short label on screen and the full name for screen readers. */
const LANGUAGES_CONFIG = [
	{ code: LANGUAGES_SHORT.UK, label: "UA", name: "Українська" },
	{ code: LANGUAGES_SHORT.EN, label: "EN", name: "English" },
];

interface LanguageSwitcherProps {
	language: string;
}

/** Toggle between the supported languages; the choice is remembered in a cookie. */
const LanguageSwitcher = ({ language }: Readonly<LanguageSwitcherProps>) => {
	const t = getI18n(language, LANGUAGE_SWITCHER_I18N);

	return (
		<ToggleButtonGroup
			aria-label={t("change_language")}
			selectionMode="single"
			disallowEmptySelection
			selectedKeys={[language]}
			onSelectionChange={(keys) => {
				const [selected] = keys;

				if (typeof selected === "string" && selected !== language) {
					void setLanguage(selected);
				}
			}}
			className="inline-flex rounded-lg border border-border bg-surface p-0.5"
		>
			{LANGUAGES_CONFIG.map((item) => (
				<ToggleButton
					key={item.code}
					id={item.code}
					aria-label={item.name}
					lang={item.code}
					className="cursor-pointer rounded-md px-3 py-1 font-medium text-sm text-text-muted outline-none transition-colors data-selected:bg-primary data-hovered:not-data-selected:bg-muted data-selected:text-on-primary data-focus-visible:ring-2 data-focus-visible:ring-focus"
				>
					{item.label}
				</ToggleButton>
			))}
		</ToggleButtonGroup>
	);
};

export default LanguageSwitcher;
