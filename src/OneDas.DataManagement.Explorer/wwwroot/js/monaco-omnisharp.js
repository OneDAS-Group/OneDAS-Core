window.monacoEditors = [];

function CreateMonacoEditor(editorId, options) {
    var editor = monaco.editor.create(document.getElementById("monaco-editor"), options);
    window.monacoEditors.push({ id: editorId, editor: editor });
}

function RegisterMonacoProviders(editorId, filterEditor, monacoService) {

    // document semantic tokens provider
    //window.monaco.languages.registerDocumentSemanticTokensProvider("csharp", {
    //    provideDocumentSemanticTokens: (model, lastResultId) => {
    //        return this.provideDocumentSemanticTokens(item, lastResultId, monacoService)
    //    }
    //});
    
    // completionItem provider
    window.monaco.languages.registerCompletionItemProvider("csharp", {
        triggerCharacters: ["."],
        resolveCompletionItem: (item) => {
            return this.resolveCompletionItem(item, monacoService)
        },
        provideCompletionItems: (model, position, context) => {
            return this.provideCompletionItems(model, position, context, monacoService)
        }
    });

    // signatureHelp provider
    window.monaco.languages.registerSignatureHelpProvider("csharp", {
        signatureHelpTriggerCharacters: ['('],
        provideSignatureHelp: (model, position) => {
            return this.provideSignatureHelp(model, position, monacoService)
        }
    });

    // hover provider
    window.monaco.languages.registerHoverProvider("csharp", {
        provideHover: (model, position) => {
            return this.provideHover(model, position, monacoService)
        }
    });

    // diagnostics
    var editor = this._getEditor(editorId);

    editor.onDidChangeModelContent(async e => {

        var code = editor.getValue();
        await filterEditor.invokeMethodAsync("UpdateCodeAsync", code);
    })
}

function SetMonacoValue(editorId, value) {
    var editor = this._getEditor(editorId);
    editor.setValue(value);
}

function GetMonacoValue(editorId, value) {
    var editor = this._getEditor(editorId);
    return editor.getValue();
}

function SetMonacoDiagnostics(editorId, diagnostics) {

    var editor = this._getEditor(editorId);

    diagnostics.forEach(diagnostic => {
        diagnostic.startLineNumber = diagnostic.start.line + 1;
        diagnostic.startColumn = diagnostic.start.character + 1;

        diagnostic.endLineNumber = diagnostic.end.line + 1;
        diagnostic.endColumn = diagnostic.end.character + 1;
    });

    window.monaco.editor.setModelMarkers(editor.getModel(), "owner", diagnostics);
}

function _getEditor(editorId)
{
    let editorEntry = window.monacoEditors.find(editor => editor.id === editorId);

    if (!editorEntry) {
        throw `Could not find editor with id ${editorId}.`;
    }

    return editorEntry.editor;
}

/* MIT License
 *
 * Copyright (c) .NET Foundation and Contributors All Rights Reserved
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

let _lastCompletions;

async function provideDocumentSemanticTokens(item, lastResultId, monacoService) {
    // todo: implement https://github.com/OmniSharp/omnisharp-vscode/blob/master/src/features/semanticTokensProvider.ts
}

async function provideCompletionItems(model, position, context, monacoService) {
    
    let request = this._createRequest(position);
    request.CompletionTrigger = (context.triggerKind + 1);
    request.TriggerCharacter = context.triggerCharacter;

    try {

        let code = model.getValue();
        const response = await monacoService.invokeMethodAsync("GetCompletionAsync", code, request);
        const mappedItems = response.items.map(this._convertToVscodeCompletionItem);

        let lastCompletions = new Map();

        for (let i = 0; i < mappedItems.length; i++) {
            lastCompletions.set(mappedItems[i], response.items[i]);
        }

        this._lastCompletions = lastCompletions;
        
        return { suggestions: mappedItems };
    }
    catch (error) {
        console.log(error);
        return;
    }
}

async function resolveCompletionItem(item, monacoService) {
    
    const lastCompletions = this._lastCompletions;

    if (!lastCompletions) {
        return item;
    }

    const lspItem = lastCompletions.get(item);

    if (!lspItem) {
        return item;
    }

    const request = { Item: lspItem };

    try {
        const response = await monacoService.invokeMethodAsync("GetCompletionResolveAsync", request);
        return this._convertToVscodeCompletionItem(response.item);
    }
    catch (error) {
        console.log(error);
        return;
    }
}

async function provideSignatureHelp(model, position, monacoService) {
    let req = this._createRequest(position);

    try {

        let code = model.getValue();
        let res = await monacoService.invokeMethodAsync("GetSignatureHelpAsync", code, req);

        if (!res) {
            return undefined;
        }

        let ret = {
            signatures: [],
            activeSignature: res.activeSignature,
            activeParameter: res.activeParameter
        }

        for (let signature of res.signatures) {

            let signatureInfo = {
                label: signature.label,
                documentation: signature.structuredDocumentation.summaryText,
                parameters: []
            }

            ret.signatures.push(signatureInfo);

            for (let parameter of signature.parameters) {
                let parameterInfo = {
                    label: parameter.label,
                    documentation: this._getParameterDocumentation(parameter)
                }

                signatureInfo.parameters.push(parameterInfo);
            }
        }

        return {
            value: ret,
            dispose: () => { }
        }
    }
    catch (error) {
        console.log(error);
        return undefined;
    }
}

async function provideHover(document, position, monacoService) {

    let request = this._createRequest(position);
    try {
        const response = await monacoService.invokeMethodAsync("GetQuickInfoAsync", request);
        if (!response || !response.markdown) {
            return undefined;
        }

        return {
            contents: [
                {
                    value: response.markdown
                }
            ]
        }
    }
    catch (error) {
        console.log(error);
        return undefined;
    }
}

function _getParameterDocumentation(parameter) {
    let summary = parameter.documentation;

    if (summary.length > 0) {
        let paramText = `**${parameter.name}**: ${summary}`;
        return {
            value: paramText
        };
    }

    return "";
}

function _convertToVscodeCompletionItem(omnisharpCompletion) {
    const docs = omnisharpCompletion.documentation;

    const mapRange = function (edit) {
        const newStart = {
            lineNumber: edit.startLine + 1,
            column: edit.startColumn + 1
        };
        const newEnd = {
            lineNumber: edit.endLine + 1,
            column: edit.endColumn + 1
        };
        return {
            startLineNumber: newStart.lineNumber,
            startColumn: newStart.column,
            endLineNumber: newEnd.lineNumber,
            endColumn: newEnd.column
        };
    };

    const mapTextEdit = function (edit) {
        return new TextEdit(mapRange(edit), edit.NewText);
    };

    const additionalTextEdits = omnisharpCompletion.additionalTextEdits?.map(mapTextEdit);

    const newText = omnisharpCompletion.textEdit?.newText ?? omnisharpCompletion.insertText;
    const insertText = newText;

    const insertRange = omnisharpCompletion.textEdit ? mapRange(omnisharpCompletion.textEdit) : undefined;

    return {
        label: omnisharpCompletion.label,
        kind: omnisharpCompletion.kind - 1,
        detail: omnisharpCompletion.detail,
        documentation: {
            value: docs
        },
        commitCharacters: omnisharpCompletion.commitCharacters,
        preselect: omnisharpCompletion.preselect,
        filterText: omnisharpCompletion.filterText,
        insertText: insertText,
        range: insertRange,
        tags: omnisharpCompletion.tags,
        sortText: omnisharpCompletion.sortText,
        additionalTextEdits: additionalTextEdits,
        keepWhitespace: true
    };
}

function _createRequest(position) {

    let Line, Column;
    
    Line = position.lineNumber - 1;
    Column = position.column - 1;

    let request = {
        Line,
        Column
    };

    return request;
}