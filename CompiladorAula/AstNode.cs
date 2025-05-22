using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public abstract class AstNode
    {
        public abstract T Accept<T>(IAstVisitor<T> visitor);
    }

    public abstract class Expr : AstNode
    { 
    }



    // Binary expression (e.g., a + b, x * y)
    public class BinaryExpr : Expr 
    {
        public Expr Left { get; }
        public Token Operator { get; }
        public Expr Right { get; }

        public BinaryExpr(Expr left, Token op, Expr right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    // Literal expression (e.g., 42, "hello")
    public class LiteralExpr : Expr
    {
        public object Value { get; }
        public TokenTypes Type { get; }

        public LiteralExpr(object value, TokenTypes type)
        {
            Value = value;
            Type = type;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    // Variable reference expression (e.g., x, counter)
    public class VariableExpr : Expr
    {
        public Token Name { get; }

        public VariableExpr(Token name)
        {
            Name = name;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitVariableExpr(this);
        }
    }

    // Assignment expression (e.g., x = 10)
    public class AssignExpr : Expr
    {
        public Token Name { get; }
        public Expr Value { get; }

        public AssignExpr(Token name, Expr value)
        {
            Name = name;
            Value = value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitAsignExpr(this);
        }
    }

    // Statement nodes
    public abstract class Stmt : AstNode
    {
    }

    // Expression statement (an expression followed by a semicolon)
    public class ExpressionStmt : Stmt
    {
        public Expr Expression { get; }

        public ExpressionStmt(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitExpressionStmt(this);
        }
    }

    // Print statement (e.g., print x;)
    public class PrintStmt : Stmt
    {
        public Expr Expression { get; }

        public PrintStmt(Expr expression)
        {
            Expression = expression;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitPrintStmt(this);
        }
    }


    // Variable declaration statement (e.g., int x = 10;)
    public class VarDeclarationStmt : Stmt
    {
        public Token Name { get; }
        public TokenTypes Type { get; }
        public Expr Initializer { get; }

        public VarDeclarationStmt(Token name, TokenTypes type, Expr initializer)
        {
            Name = name;
            Type = type;
            Initializer = initializer;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitVarDeclarationStmt(this);
        }
    }

    // Block statement (a sequence of statements)
    public class BlockStmt : Stmt
    {
        public List<Stmt> Statements { get; }

        public BlockStmt(List<Stmt> statements)
        {
            Statements = statements;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitBlockStmt(this);
        }
    }

    // If statement
    public class IfStmt : Stmt
    {
        public Expr Condition { get; }
        public Stmt ThenBranch { get; }
        public Stmt ElseBranch { get; }

        public IfStmt(Expr condition, Stmt thenBranch, Stmt elseBranch)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitIfStmt(this);
        }
    }

    // While statement
    public class WhileStmt : Stmt
    {
        public Expr Condition { get; }
        public Stmt Body { get; }

        public WhileStmt(Expr condition, Stmt body)
        {
            Condition = condition;
            Body = body;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitWhileStmt(this);
        }
    }
}
