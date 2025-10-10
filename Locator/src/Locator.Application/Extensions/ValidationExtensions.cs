﻿using FluentValidation.Results;
using Shared;

namespace Locator.Application.Extensions;

public static class ValidationExtensions
{
    public static Error[] ToErrors(this ValidationResult validationResult) =>
        validationResult.Errors.Select(e => Error.Validation(e.ErrorMessage, invalidField: e.PropertyName, code: e.ErrorCode)).ToArray();
}