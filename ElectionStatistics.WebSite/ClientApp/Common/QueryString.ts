import { IStringDictionary } from ".";

export function stringify (object: any, prefix?: string): string {
	const str = [];
	let hasProperties = false;
	if (typeof object != "string") {
		for (let property in object) {
			if (object.hasOwnProperty(property)) {
				hasProperties = true;
				let key = prefix ? prefix + "." + property : property;
				let value = object[property];

				if (Object.prototype.toString.call(value) === "[object Array]") {
					for (let i = 0; i < value.length; i++) {
						str.push(stringify(value[i], key + "[" + i + "]"));
					}
				}
				else if (typeof value === "object") {
					str.push(stringify(value, key));
				}
				else if (typeof value !== "undefined") {
					str.push(encodeURIComponent(key) + "=" + encodeURIComponent(value));
				}
			}
		}
	}
	if (!hasProperties
		&& typeof object !== "undefined"
		&& object !== null) {
		str.push(encodeURIComponent(prefix ? prefix + "=" : "") + encodeURIComponent(object));
	}
	return str.filter(s => s).join("&");
};

export function parse<TResult> (queryString: string): TResult {
	const result = {} as IStringDictionary<any>;

	if (queryString != null && queryString != "") {
		const queryStringPairs = queryString
			.substring(1)
			.split("&")
			.map(keyValuePair => keyValuePair.split("="));
		
		for (let queryStringPair of queryStringPairs) {
			const keys = queryStringPair[0].split(".");
			const stringValue = queryStringPair[1];
			const intValue = parseInt(stringValue);
			const floatValue = parseFloat(stringValue);
			
			let ojectForValue = result;
			for (let keyIndex = 0; keyIndex < keys.length - 1; keyIndex++){
				const key = keys[keyIndex];
				ojectForValue = result[key] as IStringDictionary<any>;
				if (ojectForValue == null) {
					ojectForValue = {} as IStringDictionary<any>;
					result[key] = ojectForValue;
				}				
			}

			ojectForValue[keys[keys.length - 1]] = intValue || floatValue || stringValue;
		}
	}

	return result as TResult;
};