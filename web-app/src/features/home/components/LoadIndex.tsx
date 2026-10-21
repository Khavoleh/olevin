"use client";

import { showErrorToast } from "@shared/components";
import { useEffect } from "react";
import { getLatestIndex } from "../actions/get-latest-index";
import { useHomeStore } from "../home-store";

interface LoadIndexProps {
	language: string;
}

/** Puts the latest index into the store. */
const LoadIndex = ({ language }: Readonly<LoadIndexProps>) => {
	const setIndex = useHomeStore((state) => state.setIndex);

	useEffect(() => {
		void getLatestIndex(language).then((result) => {
			if ("error" in result) {
				showErrorToast(result.error);
			} else {
				setIndex(result.index);
			}
		});
	}, [language, setIndex]);

	return null;
};

export default LoadIndex;
