using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAula
{
    public interface IAstVisitor<T>
    {
        T VisitBinaryExpr(BinaryExpr expr);
        T VisitLiteralExpr(LiteralExpr expr);
        T VisitVariableExpr(VariableExpr expr);
        T VisitAsignExpr(AssignExpr expr);
        T VisitExpressionStmt(ExpressionStmt stmt);
        T VisitPrintStmt(PrintStmt stmt);
        T VisitVarDeclarationStmt(VarDeclarationStmt stmt);
        T VisitBlockStmt(BlockStmt stmt);
        T VisitIfStmt(IfStmt stmt);
        T VisitWhileStmt(WhileStmt stmt);
    }
}
