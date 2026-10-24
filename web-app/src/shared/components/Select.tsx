"use client";

import { CaretDownIcon } from "@phosphor-icons/react";
import {
	Select as AriaSelect,
	type SelectProps as AriaSelectProps,
	Button,
	ListBox,
	Popover,
	SelectValue,
} from "react-aria-components";
import ComboBoxItem from "./ComboBox/ComboBoxItem";
import Label from "./Label";

interface SelectOption {
	id: string | number;
	name: string;
}

interface SelectProps
	extends Omit<AriaSelectProps<SelectOption>, "className" | "children"> {
	label: string;
	options: SelectOption[];
	className?: string;
}

/** Drop-down list with its label beside it; options are given as `id` and `name`. */
const Select = ({
	label,
	options,
	className = "",
	...props
}: Readonly<SelectProps>) => {
	return (
		<AriaSelect
			{...props}
			className={`flex items-center gap-2 outline-none ${className}`}
		>
			<Label className="text-fg-muted">{label}</Label>
			<Button className="flex min-h-11 cursor-pointer items-center gap-1.5 rounded-md border border-input bg-surface px-3 font-semibold text-fg text-sm hover:border-input-hover">
				<SelectValue />
				<CaretDownIcon size={16} aria-hidden />
			</Button>
			<Popover className="z-20 min-w-(--trigger-width) overflow-hidden rounded-lg border border-border bg-surface shadow-overlay data-entering:animate-popover-in data-exiting:animate-popover-out">
				<ListBox
					items={options}
					className="max-h-72 overflow-y-auto p-1 outline-none"
				>
					{(option) => (
						<ComboBoxItem id={option.id}>{option.name}</ComboBoxItem>
					)}
				</ListBox>
			</Popover>
		</AriaSelect>
	);
};

export default Select;
