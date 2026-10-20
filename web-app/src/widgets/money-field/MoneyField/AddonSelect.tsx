"use client";

import { CaretDownIcon } from "@phosphor-icons/react";
import { ComboBoxItem } from "@shared/components";
import {
	Button,
	ListBox,
	Popover,
	Select,
	SelectValue,
} from "react-aria-components";

interface AddonSelectProps {
	label: string;
	options: readonly string[];
	value?: string;
	onChange: (value: string) => void;
}

/** Currency of an amount, a short select inside the field box after the input. */
const AddonSelect = ({
	label,
	options,
	value,
	onChange,
}: Readonly<AddonSelectProps>) => {
	return (
		<Select
			aria-label={label}
			placeholder="—"
			value={value ?? null}
			onChange={(key) => {
				if (key) onChange(String(key));
			}}
			className="flex outline-none"
		>
			<Button className="flex cursor-pointer items-center gap-1 rounded-r-[calc(var(--radius-md)-1px)] border-border border-l bg-surface-sunken px-2.5 font-semibold text-fg text-sm -outline-offset-2 transition-colors hover:bg-surface-hover">
				<SelectValue />
				<CaretDownIcon size={14} />
			</Button>
			<Popover className="z-20 min-w-(--trigger-width) overflow-hidden rounded-lg border border-border bg-surface shadow-overlay outline-none data-entering:animate-popover-in data-exiting:animate-popover-out data-[placement=top]:[--popover-shift:4px]">
				<ListBox className="max-h-72 overflow-y-auto p-1 outline-none">
					{options.map((option) => (
						<ComboBoxItem key={option} id={option} textValue={option}>
							{option}
						</ComboBoxItem>
					))}
				</ListBox>
			</Popover>
		</Select>
	);
};

export default AddonSelect;
