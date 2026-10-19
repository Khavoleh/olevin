"use client";

import { CaretDownIcon } from "@phosphor-icons/react";
import type { ReactNode } from "react";
import {
	ComboBox as AriaComboBox,
	type ComboBoxProps as AriaComboBoxProps,
	Button,
	ListBox,
	Popover,
} from "react-aria-components";
import FieldError from "../FieldError";
import Label from "../Label";
import Text from "../Text";
import ComboBoxInput from "./ComboBoxInput";

interface ComboBoxProps<T extends object>
	extends Omit<AriaComboBoxProps<T>, "className" | "children"> {
	label: string;
	buttonLabel: string;
	description?: string;
	errorMessage?: string;
	labelVariant?: "field" | "group";
	className?: string;
	children: ReactNode | ((item: T) => ReactNode);
}

/** Searchable select with its label, hint and validation error; options are `ComboBoxItem`s. */
const ComboBox = <T extends object>({
	label,
	buttonLabel,
	description,
	errorMessage,
	labelVariant,
	className = "",
	children,
	...props
}: Readonly<ComboBoxProps<T>>) => {
	return (
		<AriaComboBox {...props} className={`grid gap-1.5 ${className}`}>
			<Label variant={labelVariant}>{label}</Label>
			<div className="relative">
				<ComboBoxInput />
				<Button
					aria-label={buttonLabel}
					className="absolute inset-y-0 right-0 flex w-11 cursor-pointer items-center justify-center text-fg-muted outline-none"
				>
					<CaretDownIcon size={18} />
				</Button>
			</div>
			{description && (
				<Text slot="description" variant="hint">
					{description}
				</Text>
			)}
			<FieldError>{errorMessage}</FieldError>
			<Popover className="w-(--trigger-width) overflow-hidden rounded-lg border border-border bg-surface shadow-overlay data-entering:animate-popover-in data-exiting:animate-popover-out data-[placement=top]:[--popover-shift:4px]">
				<ListBox className="max-h-72 overflow-y-auto p-1 outline-none">
					{children}
				</ListBox>
			</Popover>
		</AriaComboBox>
	);
};

export default ComboBox;
