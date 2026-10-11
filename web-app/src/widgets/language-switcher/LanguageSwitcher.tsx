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
			className="inline-flex rounded-md border border-input bg-surface p-0.5"
		>
			{LANGUAGES_CONFIG.map((item) => (
				<ToggleButton
					key={item.code}
					id={item.code}
					aria-label={item.name}
					lang={item.code}
					className={({ isSelected, isHovered, isFocusVisible }) => {
						const colors = isSelected
							? "bg-primary text-on-primary"
							: `text-fg-muted ${isHovered ? "bg-surface-hover" : ""}`;
						const focus = isFocusVisible ? "ring-2 ring-ring" : "";

						return `min-h-10 cursor-pointer rounded-sm px-3 font-medium text-sm outline-none transition-colors ${colors} ${focus}`;
					}}
				>
					{item.label}
				</ToggleButton>
			))}
		</ToggleButtonGroup>
	);
};

export default LanguageSwitcher;
