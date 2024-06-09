

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using Xunit.Sdk;
//import org.hamcrest.Matcher;

//import java.util.ArrayList;
//import java.util.List;
//import java.util.concurrent.atomic.AtomicInteger;

//import static org.hamcrest.MatcherAssert.assertThat;

/**
 * AssertionGroup groups multiple assertion into a group.  All of the assertion within the group is executed even if one
 * or more assertions fail.  At the end of the group, all accumulated assertion error, if any, is thrown in one chunk.
 *
 * The traditional assertion always interrupts the test as soon as a failure is encountered, leaving the rest of
 * assertions untested.  When testing multiple values this is not always desirable.  It would mean that if there are
 * more than two errors on a single page or single set of data, only the first error is captured, and other errors
 * would remain unknown until the first one is fixed.
 *
 * Grouping makes it possible to get the multiple assertion failures captured at one time, potentially saving time on
 * finding multiple issues.
 *
 * @author yasuim
 * @since 8/3/2018
 */
namespace Automation_Framework.Utilities
{
    public interface AssertionGroup
    {
        /**
         * Group multiple assertion using lambda expression.
         * @param assertionGroup
         */
        static void group(List<Action> assertionGroup)
        {
            group(null, assertionGroup);
        }

        /**
         * Group multiple assertion using lambda expression.
         * @param description
         * @param assertionGroup
         */
        static void group(String description, List<Action> assertionGroup)
        {
            Group g = new Group(description);

            try
            {
                AssertionGroupLevel.incrementLevel();
                g.group(assertionGroup);
            }
            catch (Xunit.Sdk.XunitException ex)
            {
                g.assertionErrors.Add(ex);
            }
            finally
            {
                AssertionGroupLevel.decrementLevel();
                g.handleAssertionErrors();
            }
        }

        /**
         * Keep track of assertion group level depth.
         */
        class AssertionGroupLevel
        {
            private static ThreadLocal<int?> level = new ThreadLocal<int?>();

            internal static void incrementLevel()
            {
                if (level.IsValueCreated == false)
                {
                    level.Value = 1;
                }
                else
                {
                    level.Value++;
                }
            }

            internal static void decrementLevel()
            {
                if (level.IsValueCreated == false)
                {
                    level.Value = -1;
                }
                else
                {
                    level.Value--;
                    
                }
            }

            static int getLevel()
            {
                if (level.IsValueCreated == false)
                {
                    return 0;
                }
                else
                {
                    return (int)level.Value;
                }
            }

            /**
             * Create padding according to depth level.
             * @return
             */
            internal static String getPadding()
            {
                String padding = "";

                for (int i = 0; i < getLevel(); i++)
                {
                    padding += "  ";
                }
                return padding;
            }

            private AssertionGroupLevel() { }
        }

        // Declare a delegate Type for processing asserts

        /**
         * Assertion group class.
         */
        protected class Group
        {
            private String description;
            public List<XunitException> assertionErrors;
            //private List<Group> children = new List<Group>();
           

            internal Group(string description)
            {
                this.description = description;
                this.assertionErrors = new List<XunitException>();
            }

            public void group(List<Action> assertionGroupParms)
            {
                try
                {
                    AssertionGroupLevel.incrementLevel();
                    assertAll(assertionGroupParms);
                }
                catch (XunitException ex)
                {
                    assertionErrors.Add(ex);
                }
                finally
                {
                    AssertionGroupLevel.decrementLevel();
                 
                }

            }

            protected void assertAll(List<Action> group)
            {
                foreach (Action a in group)
                {
                    try
                    {
                        a();
                    }
                    catch (XunitException ex)
                    {
                        this.assertionErrors.Add(ex);
                    }
                }
            }

            /**
             * Handle assertion errors.
             */
            internal void handleAssertionErrors()
            {
                FEGroupAssertExceptions newAssertionError = new FEGroupAssertExceptions(getGroupAssertionErrorDescription(this, AssertionGroupLevel.getPadding()));
                List<XunitException> errors = getAssertionErrors(this);
               
                if (errors.Count > 0)
                {
                    newAssertionError.Add(errors);
                    throw newAssertionError;
                }
            }

            /**
             * Recursively get all assertion errors.
             * @param g
             * @return
             */
            private static List<XunitException> getAssertionErrors(Group g)
            {
                return g.assertionErrors;
            }

            /**
             * Get group assertion error description.
             * @param g
             * @param padding
             * @return
             */
            private static String getGroupAssertionErrorDescription(Group g, String padding)
            {
                String errorDescription = "";

                if (g.assertionErrors.Count > 0)
                {
                    errorDescription = "One or more assertion errors were found in assertion group."; 

                    errorDescription += "\n";
                    errorDescription += padding + "-----------------------\n";
                    foreach (XunitException error in g.assertionErrors)
                    {
                        String message = error.Message;

                        if (message.Length > 360)
                        {
                            message = message.Substring(0, 360) + " ...(truncated)";
                        }
                        errorDescription += padding + message + "\n";
                    }
                    errorDescription += padding + "=======================\n";
                }
               
                return errorDescription;
            }
        }
    }

    public class FEGroupAssertExceptions : XunitException
    {
        public List<XunitException> ExceptionList { get; protected set; }
        //
        // Summary:
        //     Initializes a new instance of the Xunit.Sdk.XunitException class.
        public FEGroupAssertExceptions() : base()
        { }
        //
        // Summary:
        //     Initializes a new instance of the Xunit.Sdk.XunitException class.
        //
        // Parameters:
        //   userMessage:
        //     The user message to be displayed
        public FEGroupAssertExceptions(string userMessage) : base(userMessage)
        { }
        //
        // Summary:
        //     Initializes a new instance of the Xunit.Sdk.XunitException class.
        //
        // Parameters:
        //   userMessage:
        //     The user message to be displayed
        //
        //   innerException:
        //     The inner exception
        protected FEGroupAssertExceptions(string userMessage, Exception innerException) : base (userMessage, innerException)
        { }
        //
        // Summary:
        //     Initializes a new instance of the Xunit.Sdk.XunitException class.
        //
        // Parameters:
        //   userMessage:
        //     The user message to be displayed
        //
        //   stackTrace:
        //     The stack trace to be displayed
        protected FEGroupAssertExceptions(string userMessage, string stackTrace) : base (userMessage, stackTrace)
        { }

        public void Add(XunitException exception)
        {
            if (ExceptionList == null)
            {
                ExceptionList = new List<XunitException>();
            }
            ExceptionList.Add(exception);
        }

        public void Add(List<XunitException> exceptions)
        {
            if (ExceptionList == null)
            {
                ExceptionList = new List<XunitException>();
            }
            ExceptionList.AddRange(exceptions);
        }

    }

}