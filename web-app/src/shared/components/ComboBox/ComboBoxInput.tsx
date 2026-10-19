"use client";

import { useContext } from "react";
import { ComboBoxStateContext } from "react-aria-components";
import Input from "../Input";

/** Input of a `ComboBox` that opens the full list on click. */
const ComboBoxInput = () => {
	const state = useContext(ComboBoxStateContext);

	return (
		<Input
			className="pr-11"
			onClick={() => {
				if (state && !state.isOpen) state.open(null, "manual");
			}}
		/>
	);
};

export default ComboBoxInput;
