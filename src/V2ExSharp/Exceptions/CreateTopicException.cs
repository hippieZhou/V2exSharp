using System;
using V2exSharp.Models;

namespace V2exSharp.Exceptions;

public class CreateTopicException(Problem problem) : Exception
{
    public Problem Problem { get; } = problem;
}
