"use client";

import type { ReactNode } from "react";
import {
	Input as AriaInput,
	NumberField as AriaNumberField,
	type NumberFieldProps as AriaNumberFieldProps,
	Group,
} from "react-aria-components";
import FieldError from "./FieldError";
import Input from "./Input";
import Label from "./Label";
import Text from "./Text";

interface NumberFieldProps extends Omit<AriaNumberFieldProps, "className"> {
	label: string;
	description?: string;
	errorMessage?: string;
	labelVariant?: "field" | "group";
	placeholder?: string;
	addon?: ReactNode;
	className?: string;
	inputClassName?: string;
}

const GROUP_STATE_CLASSES = {
	idle: "border-input",
	hovered: "border-input-hover",
	invalid: "border-input-invalid",
};

/** Number field with its label, hint and validation error, formatted for the locale; `addon` sits inside the box after the input. */
const NumberField = ({
	label,
	description,
	errorMessage,
	labelVariant,
	placeholder,
	addon,
	className = "",
	inputClassName = "",
	...props
}: Readonly<NumberFieldProps>) => {
	return (
		<AriaNumberField {...props} className={`grid gap-1.5 ${className}`}>
			<Label variant={labelVariant}>{label}</Label>
			{addon ? (
				<Group
					className={({ isHovered, isInvalid }) => {
						const border =
							(isInvalid && GROUP_STATE_CLASSES.invalid) ||
							(isHovered && GROUP_STATE_CLASSES.hovered) ||
							GROUP_STATE_CLASSES.idle;

						return `flex min-h-11 min-w-0 rounded-md border bg-surface outline-transparent transition-colors has-[>input:focus-visible]:outline-ring ${border} ${inputClassName}`;
					}}
				>
					<AriaInput
						placeholder={placeholder}
						className="w-0 min-w-0 flex-1 bg-transparent px-3 text-fg tabular-nums outline-none"
					/>
					{addon}
				</Group>
			) : (
				<Input placeholder={placeholder} className={inputClassName} />
			)}
			{description && (
				<Text slot="description" variant="hint">
					{description}
				</Text>
			)}
			<FieldError>{errorMessage}</FieldError>
		</AriaNumberField>
	);
};

export default NumberField;
