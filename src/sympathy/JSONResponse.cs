
/*
 * This file is part of Jkop for .NET
 * Copyright (c) 2016-2017 Job and Esther Technologies, Inc.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace sympathy
{
	public class JSONResponse
	{
		public static cape.DynamicMap forError(cape.Error error) {
			var v = new cape.DynamicMap();
			v.set("status", (object)"error");
			if(error != null) {
				var code = error.getCode();
				var message = error.getMessage();
				var detail = error.getDetail();
				if(cape.String.isEmpty(code) == false) {
					v.set("code", (object)code);
				}
				if(cape.String.isEmpty(message) == false) {
					v.set("message", (object)message);
				}
				if(cape.String.isEmpty(detail) == false) {
					v.set("detail", (object)detail);
				}
			}
			return(v);
		}

		public static cape.DynamicMap forErrorCode(string code) {
			var v = new cape.DynamicMap();
			v.set("status", (object)"error");
			v.set("code", (object)code);
			return(v);
		}

		public static cape.DynamicMap forErrorMessage(string message = null, string code = null) {
			var v = new cape.DynamicMap();
			v.set("status", (object)"error");
			if(cape.String.isEmpty(message) == false) {
				v.set("message", (object)message);
			}
			if(cape.String.isEmpty(code) == false) {
				v.set("code", (object)code);
			}
			return(v);
		}

		public static cape.DynamicMap forOk(object data = null) {
			var v = new cape.DynamicMap();
			v.set("status", (object)"ok");
			if(data != null) {
				v.set("data", data);
			}
			return(v);
		}

		public static cape.DynamicMap forDetails(string status, string code, string message) {
			var v = new cape.DynamicMap();
			if(cape.String.isEmpty(status) == false) {
				v.set("status", (object)status);
			}
			if(cape.String.isEmpty(code) == false) {
				v.set("code", (object)code);
			}
			if(cape.String.isEmpty(message) == false) {
				v.set("message", (object)message);
			}
			return(v);
		}

		public static cape.DynamicMap forMissingData(string type = null) {
			if(cape.String.isEmpty(type) == false) {
				return(forErrorMessage("Missing data: " + type, "missing_data"));
			}
			return(forErrorMessage("Missing data", "missing_data"));
		}

		public static cape.DynamicMap forInvalidData(string type = null) {
			if(cape.String.isEmpty(type) == false) {
				return(forErrorMessage("Invalid data: " + type, "invalid_data"));
			}
			return(forErrorMessage("Invalid data", "invalid_data"));
		}

		public static cape.DynamicMap forAlreadyExists() {
			return(forErrorMessage("Already exists", "already_exists"));
		}

		public static cape.DynamicMap forInvalidRequest(string type = null) {
			if(cape.String.isEmpty(type) == false) {
				return(forErrorMessage("Invalid request: " + type, "invalid_request"));
			}
			return(forErrorMessage("Invalid request", "invalid_request"));
		}

		public static cape.DynamicMap forNotAllowed() {
			return(forErrorMessage("Not allowed", "not_allowed"));
		}

		public static cape.DynamicMap forFailedToCreate() {
			return(forErrorMessage("Failed to create", "failed_to_create"));
		}

		public static cape.DynamicMap forNotFound() {
			return(forErrorMessage("Not found", "not_found"));
		}

		public static cape.DynamicMap forAuthenticationFailed() {
			return(forErrorMessage("Authentication failed", "authentication_failed"));
		}

		public static cape.DynamicMap forIncorrectUsernamePassword() {
			return(forErrorMessage("Incorrect username and/or password", "authentication_failed"));
		}

		public static cape.DynamicMap forInternalError(string details = null) {
			if(cape.String.isEmpty(details) == false) {
				return(forErrorMessage("Internal error: " + details, "internal_error"));
			}
			return(forErrorMessage("Internal error", "internal_error"));
		}
	}
}
